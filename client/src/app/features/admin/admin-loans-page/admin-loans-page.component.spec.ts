import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminLoansPageComponent } from './admin-loans-page.component';

describe('AdminLoansPageComponent', () => {
  let component: AdminLoansPageComponent;
  let fixture: ComponentFixture<AdminLoansPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminLoansPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminLoansPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
