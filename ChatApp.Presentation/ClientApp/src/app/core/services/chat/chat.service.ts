import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import { ChatRoom, MessageDto } from '../../models/chat.models';
import { AuthService } from '../auth/auth.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;

  // State management for real-time messages
  private messagesSubject = new BehaviorSubject<MessageDto[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  // Observable for real-time room creation
  private roomCreatedSubject = new Subject<ChatRoom>();
  public roomCreated$ = this.roomCreatedSubject.asObservable();

  constructor(
    private http: HttpClient,
    private zone: NgZone,
    private authService: AuthService
  ) {}

  // --- HTTP API CALLS ---
  getRooms(): Observable<ChatRoom[]> {
    return this.http.get<ChatRoom[]>('/api/chatrooms');
  }

  createRoom(name: string): Observable<ChatRoom> {
    return this.http.post<ChatRoom>('/api/chatrooms', JSON.stringify(name), {
      headers: { 'Content-Type': 'application/json' }
    });
  }

  getMessagesHistory(roomId: string): Observable<MessageDto[]> {
    return this.http.get<MessageDto[]>(`/api/messages/${roomId}`);
  }

  sendMessage(content: string, roomId: string, userId: string, username: string, roomName: string) {
    const command = { content, chatRoomId: roomId, userId, username, roomName };
    return this.http.post('/api/messages', command);
  }

  // --- SIGNALR CONFIGURATION ---
  public startConnection() {
    // Prevent multiple connections
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/chatHub', {
        // Automatically fetches the fresh token from AuthService for the WebSocket connection
        accessTokenFactory: () => this.authService.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connection Started'))
      .catch(err => console.error('Error while starting connection: ' + err));

    // Listen for incoming messages
    this.hubConnection.on('ReceiveMessage', (message: MessageDto) => {
      this.zone.run(() => {
        const currentMessages = this.messagesSubject.value;

        // BUG FIX: Deduplication. Only add the message if it doesn't already exist.
        if (!currentMessages.some(m => m.id === message.id)) {
          this.messagesSubject.next([...currentMessages, message]);
        }
      });
    });

    // Listen for real-time room creation
    this.hubConnection.on('RoomCreated', (room: ChatRoom) => {
      this.zone.run(() => {
        this.roomCreatedSubject.next(room);
      });
    });
  }

  public joinRoom(roomName: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('JoinRoom', roomName);
    }
  }

  public leaveRoom(roomName: string) {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('LeaveRoom', roomName);
    }
  }

  public setInitialMessages(messages: MessageDto[]) {
    this.messagesSubject.next(messages);
  }
}
