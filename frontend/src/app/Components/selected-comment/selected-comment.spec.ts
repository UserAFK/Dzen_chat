import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectedCommentComponent } from './selected-comment';

describe('SelectedComment', () => {
  let component: SelectedCommentComponent;
  let fixture: ComponentFixture<SelectedCommentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SelectedCommentComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SelectedCommentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
