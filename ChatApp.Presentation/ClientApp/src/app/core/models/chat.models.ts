export interface User {
  id: string;
  username: string;
}

export interface ChatRoom {
  id: string;
  name: string;
  createdAt: string;
  participants: User[];
}

export interface MessageDto {
  id: string;
  content: string;
  username: string;
  timestamp: string;
  sentiment: string;
  userId: string;
}
