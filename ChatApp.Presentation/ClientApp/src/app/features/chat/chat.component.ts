import { Component, OnInit, ChangeDetectorRef, ViewChild, ElementRef, AfterViewChecked, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../core/services/chat/chat.service';
import { AuthService } from '../../core/services/auth/auth.service';
import { ChatRoom, MessageDto, User } from '../../core/models/chat.models';
import { AuthResponseDto } from '../../core/models/auth.models';
import { Subscription } from 'rxjs';
import { MessageBubbleComponent } from './message-bubble/message-bubble.component';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, MessageBubbleComponent],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, AfterViewChecked, OnDestroy {
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;

  rooms: ChatRoom[] = [];
  selectedRoom: ChatRoom | null = null;
  messages: MessageDto[] = [];
  newMessage = '';
  newRoomName = '';
  connectionStatus = 'Disconnected';

  currentUser!: AuthResponseDto;

  isRoomsLoading = false;
  isMessagesLoading = false;

  showInviteModal = false;
  allUsers: User[] = [];
  selectedUserIds: Set<string> = new Set();

  // UI validation / error state
  roomNameError = '';
  messageError = '';
  createRoomError = '';
  sendMessageError = '';

  private subs = new Subscription();

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const user = this.authService.currentUserValue;
    if (user) {
      this.currentUser = user;
    }

    this.chatService.startConnection();
    this.loadRooms();
    this.loadUsers();

    this.subs.add(this.chatService.messages$.subscribe(msgs => {
      this.messages = msgs;
      this.cdr.detectChanges();
    }));

    this.subs.add(this.chatService.roomCreated$.subscribe(room => {
      if (!this.rooms.some(r => r.id === room.id)) {
        this.rooms.push(room);
        this.cdr.detectChanges();
      }
    }));

    this.subs.add(this.chatService.connectionStatus$.subscribe(status => {
      this.connectionStatus = status;
      this.cdr.detectChanges();
    }));
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  private scrollToBottom(): void {
    try {
      if (this.scrollContainer) {
        this.scrollContainer.nativeElement.scrollTop =
          this.scrollContainer.nativeElement.scrollHeight;
      }
    } catch (err) {}
  }

  loadRooms() {
    this.isRoomsLoading = true;
    this.chatService.getRooms().subscribe({
      next: rooms => {
        this.rooms = rooms;
        this.isRoomsLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isRoomsLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadUsers() {
    this.chatService.getUsers().subscribe(users => {
      const current = this.authService.currentUserValue;
      this.allUsers = users.filter(u => u.id !== current?.id);
    });
  }

  selectRoom(room: ChatRoom) {
    if (this.selectedRoom) {
      this.chatService.leaveRoom(this.selectedRoom.name);
    }
    this.selectedRoom = room;
    this.chatService.joinRoom(room.name);

    this.isMessagesLoading = true;
    this.chatService.getMessagesHistory(room.id).subscribe({
      next: msgs => {
        this.chatService.setInitialMessages(msgs);
        this.isMessagesLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isMessagesLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  initiateCreateRoom() {
    this.roomNameError = '';
    this.createRoomError = '';

    const name = this.newRoomName.trim();
    if (!name) {
      this.roomNameError = 'Room name is required.';
      return;
    }
    if (name.length > 100) {
      this.roomNameError = 'Room name must be 100 characters or less.';
      return;
    }

    this.showInviteModal = true;
  }

  toggleUserSelection(userId: string) {
    if (this.selectedUserIds.has(userId)) {
      this.selectedUserIds.delete(userId);
    } else {
      this.selectedUserIds.add(userId);
    }
  }

  confirmCreateRoom() {
    this.createRoomError = '';
    const name = this.newRoomName.trim();
    if (!name) {
      this.roomNameError = 'Room name is required.';
      return;
    }

    this.chatService.createRoom(name, Array.from(this.selectedUserIds))
      .subscribe({
        next: () => {
          this.newRoomName = '';
          this.selectedUserIds.clear();
          this.showInviteModal = false;
          this.cdr.detectChanges();
        },
        error: (err) => {
          this.createRoomError = err.error?.detail || err.error?.message || 'Failed to create room.';
          this.cdr.detectChanges();
        }
      });
  }

  cancelCreation() {
    this.showInviteModal = false;
    this.newRoomName = '';
    this.selectedUserIds.clear();
    this.roomNameError = '';
    this.createRoomError = '';
  }

  sendMessage() {
    this.messageError = '';
    this.sendMessageError = '';

    const user = this.authService.currentUserValue;
    const text = this.newMessage.trim();

    if (!text || !this.selectedRoom || !user) return;

    if (text.length > 500) {
      this.messageError = 'Message cannot exceed 500 characters.';
      return;
    }

    this.newMessage = '';

    this.chatService.sendMessage(
      text,
      this.selectedRoom.id,
      user.id,
      user.username,
      this.selectedRoom.name
    ).subscribe({
      error: (err) => {
        this.newMessage = text;
        this.sendMessageError = err.error?.detail || err.error?.message || 'Message failed to send.';
        this.cdr.detectChanges();
      }
    });
  }

  logout() {
    this.authService.logout();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
