import { Component, OnInit, ChangeDetectorRef, ViewChild, ElementRef, AfterViewChecked, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../core/services/chat/chat.service';
import { ChatRoom, MessageDto } from '../../core/models/chat.models';
import { MessageBubbleComponent } from './message-bubble/message-bubble.component';
import { AuthService } from '../../core/services/auth/auth.service';
import { AuthResponseDto } from '../../core/models/auth.models';
import { Subscription } from 'rxjs';

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
  activeRoom: ChatRoom | null = null;
  messages: MessageDto[] = [];

  isRoomsLoading = false;
  isMessagesLoading = false;

  newMessage = '';
  newRoomName = '';

  currentUser!: AuthResponseDto;
  private subs = new Subscription();

  constructor(
    private chatService: ChatService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    // Get the real logged-in user
    const user = this.authService.currentUserValue;
    if (user) {
      this.currentUser = user;
    }

    // Start real-time connection (Internal token handling)
    this.chatService.startConnection();
    this.loadRooms();

    // Subscribe to messages
    this.subs.add(this.chatService.messages$.subscribe(msgs => {
      this.messages = msgs;
      this.cdr.detectChanges(); // Force UI update
      this.scrollToBottom();
    }));

    // Subscribe to new rooms in real-time
    this.subs.add(this.chatService.roomCreated$.subscribe(room => {
      // BUG FIX: Only add if it's not already in the list
      if (room && !this.rooms.find(r => r.id === room.id)) {
        this.rooms.push(room);
        this.cdr.detectChanges(); // Force UI update
      }
    }));
  }

  ngOnDestroy() {
    // Cleanup to prevent memory leaks and duplicate listeners
    this.subs.unsubscribe();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      if (this.scrollContainer) {
        this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
      }
    } catch(err) { }
  }

  loadRooms() {
    this.isRoomsLoading = true;
    this.chatService.getRooms().subscribe(rooms => {
      this.rooms = rooms;
      this.isRoomsLoading = false;
      this.cdr.detectChanges();
    });
  }

  createRoom() {
    if (!this.newRoomName) return;

    // Call the API. We don't manually push here because the SignalR
    // 'RoomCreated' listener above will handle it for all users instantly.
    this.chatService.createRoom(this.newRoomName).subscribe(() => {
      this.newRoomName = '';
      this.cdr.detectChanges();
    });
  }

  selectRoom(room: ChatRoom) {
    if (this.activeRoom) {
      this.chatService.leaveRoom(this.activeRoom.name);
    }
    this.activeRoom = room;
    this.chatService.joinRoom(room.name);

    this.isMessagesLoading = true;
    this.chatService.getMessagesHistory(room.id).subscribe(history => {
      this.chatService.setInitialMessages(history);
      this.isMessagesLoading = false;
      this.cdr.detectChanges();
    });
  }

  sendMessage() {
    if (!this.newMessage || !this.activeRoom) return;

    this.chatService.sendMessage(
      this.newMessage,
      this.activeRoom.id,
      this.currentUser.id,
      this.currentUser.username,
      this.activeRoom.name
    ).subscribe();

    this.newMessage = '';
    this.cdr.detectChanges();
  }

  logout() {
    this.authService.logout();
  }
}
