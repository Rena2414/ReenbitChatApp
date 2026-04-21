import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sentiment-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span class="badge" [ngClass]="badgeClass">
      {{ sentiment }}
    </span>
  `
})
export class SentimentBadgeComponent {
  @Input() sentiment!: string;

  get badgeClass(): string {
    switch (this.sentiment?.toLowerCase()) {
      case 'positive': return 'bg-success text-white';
      case 'negative': return 'bg-danger text-white';
      default: return 'bg-secondary text-white';
    }
  }
}
