import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminStatisticsPageComponent } from './admin-statistics-page.component';

describe('AdminStatisticsPageComponent', () => {
  let component: AdminStatisticsPageComponent;
  let fixture: ComponentFixture<AdminStatisticsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminStatisticsPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminStatisticsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
