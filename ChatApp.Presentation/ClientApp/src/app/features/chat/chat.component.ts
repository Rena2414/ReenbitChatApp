import { Component, OnInit, ChangeDetectorRef } from '@angular/core'; // 1. Import ChangeDetectorRef
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../core/services/chat/chat.service';
import { ChatRoom, MessageDto } from '../../core/models/chat.models';
import { SentimentBadgeComponent } from './sentiment-badge/sentiment-badge.component';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, SentimentBadgeComponent],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  rooms: ChatRoom[] = [];
  activeRoom: ChatRoom | null = null;
  messages: MessageDto[] = [];

  isRoomsLoading = false;
  isMessagesLoading = false;

  newMessage = '';
  newRoomName = '';

  currentUser = { id: '00000000-0000-0000-0000-000000000001', username: 'TestUser' };

  // 2. Inject ChangeDetectorRef (cdr)
  constructor(private chatService: ChatService, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.chatService.startConnection();
    this.loadRooms();

    this.chatService.messages$.subscribe(msgs => {
      this.messages = msgs;
      this.cdr.detectChanges(); // 3. Force UI update on new messages
    });

    this.chatService.roomCreated$.subscribe(room => {
      if (room && !this.rooms.find(r => r.id === room.id)) {
        this.rooms.push(room);
        this.cdr.detectChanges(); // 3. Force UI update on new rooms
      }
    });
  }

  loadRooms() {
    this.isRoomsLoading = true;
    this.chatService.getRooms().subscribe(rooms => {
      this.rooms = rooms;
      this.isRoomsLoading = false;
      this.cdr.detectChanges(); // 3. Force UI update when HTTP finishes
    });
  }

  createRoom() {
    if (!this.newRoomName) return;
    this.chatService.createRoom(this.newRoomName).subscribe(() => {
      this.newRoomName = '';
      this.cdr.detectChanges(); // 3. Force UI update
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
      this.cdr.detectChanges(); // 3. Force UI update when history loads
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
    ).subscribe(() => {
      this.newMessage = '';
      this.cdr.detectChanges(); // 3. Force UI update
    });
  }
}
