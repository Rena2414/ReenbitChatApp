import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MessageDto } from '../../../core/models/chat.models';

@Component({
  selector: 'app-message-bubble',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './message-bubble.component.html',
  styleUrls: ['./message-bubble.component.css']
})
export class MessageBubbleComponent {
  @Input() message!: MessageDto;
  @Input() isCurrentUser: boolean = false;
}
