export interface User {
  id: string;
  username: string;
}

export interface ChatRoom {
  id: string;
  name: string;
  createdAt: string;
  participants: User[]; // Added for the participant list
}

export interface MessageDto { // Restored name
  id: string;
  content: string;
  username: string;
  timestamp: string;
  sentiment: string;
  userId: string;
}
