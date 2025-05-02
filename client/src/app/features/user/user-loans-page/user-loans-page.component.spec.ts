import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserLoansPageComponent } from './user-loans-page.component';

describe('UserLoansPageComponent', () => {
  let component: UserLoansPageComponent;
  let fixture: ComponentFixture<UserLoansPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserLoansPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserLoansPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
