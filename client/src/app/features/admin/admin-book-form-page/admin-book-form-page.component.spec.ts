import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminBookFormPageComponent } from './admin-book-form-page.component';

describe('AdminBookFormPageComponent', () => {
  let component: AdminBookFormPageComponent;
  let fixture: ComponentFixture<AdminBookFormPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminBookFormPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminBookFormPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
