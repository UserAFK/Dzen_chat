import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CommentListComponent } from '../comment-list/comment-list';

describe('CommentList', () => {
  let component: CommentListComponent;
  let fixture: ComponentFixture<CommentListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommentListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CommentListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
