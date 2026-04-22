export interface ChatRoom {
  id: string;
  name: string;
  createdAt: string;
}

export interface MessageDto {
  id: string;
  content: string;
  username: string;
  timestamp: string;
  sentiment: string;
  userId: string;
}
