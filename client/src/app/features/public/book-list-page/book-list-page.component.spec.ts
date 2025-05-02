import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BookListPageComponent } from './book-list-page.component';

describe('BookListPageComponent', () => {
  let component: BookListPageComponent;
  let fixture: ComponentFixture<BookListPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BookListPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BookListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
