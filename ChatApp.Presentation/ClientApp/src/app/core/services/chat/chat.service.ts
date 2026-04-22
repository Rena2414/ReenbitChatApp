import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import { ChatRoom, MessageDto, User } from '../../models/chat.models';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private hubConnection!: signalR.HubConnection;

  private messagesSubject = new BehaviorSubject<MessageDto[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  private roomCreatedSubject = new Subject<ChatRoom>();
  public roomCreated$ = this.roomCreatedSubject.asObservable();

  private connectionStatusSubject = new BehaviorSubject<string>('Disconnected');
  public connectionStatus$ = this.connectionStatusSubject.asObservable();

  constructor(
    private http: HttpClient,
    private zone: NgZone,
    private authService: AuthService
  ) {}

  getRooms(): Observable<ChatRoom[]> {
    return this.http.get<ChatRoom[]>('/api/chatrooms');
  }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('/api/users');
  }

  createRoom(name: string, participantIds: string[]): Observable<ChatRoom> {
    return this.http.post<ChatRoom>('/api/chatrooms', { roomName: name, participantIds });
  }

  getMessagesHistory(roomId: string): Observable<MessageDto[]> {
    return this.http.get<MessageDto[]>(`/api/messages/${roomId}`);
  }

  sendMessage(content: string, roomId: string, userId: string, username: string, roomName: string) {
    const command = { content, chatRoomId: roomId, userId, username, roomName };
    return this.http.post('/api/messages', command);
  }

  // --- SignalR Logic ---
  public startConnection() {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/chatHub', {
        accessTokenFactory: () => this.authService.getToken() || ''
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => {
        this.connectionStatusSubject.next('Connected');
        console.log('SignalR Connected');
      })
      .catch(err => this.connectionStatusSubject.next('Error'));

    this.hubConnection.on('ReceiveMessage', (message: MessageDto) => {
      this.zone.run(() => {
        const current = this.messagesSubject.value;
        if (!current.some(m => m.id === message.id)) {
          this.messagesSubject.next([...current, message]);
        }
      });
    });

    this.hubConnection.on('RoomCreated', (room: ChatRoom) => {
      this.zone.run(() => this.roomCreatedSubject.next(room));
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
