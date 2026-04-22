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

  // FIX 1: currentUser stored as property — template needs it for
  // the header username and msg.userId === currentUser.id comparisons.
  currentUser!: AuthResponseDto;

  // FIX 2: Loading flags — template spinners reference these but
  // they were absent from the class.
  isRoomsLoading = false;
  isMessagesLoading = false;

  showInviteModal = false;
  allUsers: User[] = [];
  selectedUserIds: Set<string> = new Set();

  private subs = new Subscription();

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // FIX 3: Resolve and store currentUser before any template renders.
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
    if (!this.newRoomName.trim()) return;
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
    this.chatService.createRoom(this.newRoomName, Array.from(this.selectedUserIds))
      .subscribe({
        next: () => {
          this.newRoomName = '';
          this.selectedUserIds.clear();
          this.showInviteModal = false;
          this.cdr.detectChanges();
        }
        // error: modal stays open so the user can retry
      });
  }

  cancelCreation() {
    this.showInviteModal = false;
    this.newRoomName = '';
    this.selectedUserIds.clear();
  }

  sendMessage() {
    const user = this.authService.currentUserValue;
    if (!this.newMessage.trim() || !this.selectedRoom || !user) return;

    // FIX 4: Capture text before clearing so the HTTP payload is correct.
    const text = this.newMessage;
    this.newMessage = ''; // Optimistic clear for instant UX feedback

    this.chatService.sendMessage(
      text,
      this.selectedRoom.id,
      user.id,
      user.username,
      this.selectedRoom.name
    ).subscribe({
      error: () => {
        // Restore the message if the send actually fails.
        this.newMessage = text;
        this.cdr.detectChanges();
      }
    });
  }

  // FIX 5: logout() was completely missing.
  logout() {
    this.authService.logout();
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
