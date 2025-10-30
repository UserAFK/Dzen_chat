import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReplyCommentComponent } from './reply-comment';

describe('ReplyComment', () => {
  let component: ReplyCommentComponent;
  let fixture: ComponentFixture<ReplyCommentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReplyCommentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReplyCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
