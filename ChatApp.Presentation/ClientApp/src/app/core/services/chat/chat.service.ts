import { Injectable, NgZone } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject, Observable } from 'rxjs'; // Import Subject
import { ChatRoom, MessageDto } from '../../models/chat.models';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection!: signalR.HubConnection;

  private messagesSubject = new BehaviorSubject<MessageDto[]>([]);
  public messages$ = this.messagesSubject.asObservable();

  // Add an observable for new rooms
  private roomCreatedSubject = new Subject<ChatRoom>();
  public roomCreated$ = this.roomCreatedSubject.asObservable();

  constructor(private http: HttpClient, private zone: NgZone) {}

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

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/chatHub')
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start()
      .then(() => console.log('SignalR Connection Started'))
      .catch(err => console.error('Error while starting connection: ' + err));

    this.hubConnection.on('ReceiveMessage', (message: MessageDto) => {
      this.zone.run(() => {
        const currentMessages = this.messagesSubject.value;
        this.messagesSubject.next([...currentMessages, message]);
      });
    });

    // Listen for incoming newly created rooms
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
