import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MessageBubbleComponent } from './message-bubble.component';

describe('MessageBubble', () => {
  let component: MessageBubbleComponent;
  let fixture: ComponentFixture<MessageBubbleComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MessageBubbleComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(MessageBubbleComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
