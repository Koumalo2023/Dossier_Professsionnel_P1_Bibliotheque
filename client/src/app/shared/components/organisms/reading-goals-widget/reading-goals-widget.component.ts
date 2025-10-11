import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserReadingGoal } from '../../../../core/models/user-profile.model';
import { CommonModule } from '@angular/common';
import { TypographyComponent } from '../../atoms/typography/typography.component';
import { HeadingComponent } from '../../atoms/heading/heading.component';
import { ProgressBarComponent } from '../../molecules/progress-bar/progress-bar.component';
import { IconComponent } from '../../atoms/icons/icon.component';
import { ButtonComponent } from '../../atoms/button/button.component';

@Component({
  selector: 'app-reading-goals-widget',
  standalone: true,
  imports: [CommonModule, TypographyComponent, HeadingComponent, ProgressBarComponent, IconComponent, ButtonComponent],
  templateUrl: './reading-goals-widget.component.html',
  styleUrl: './reading-goals-widget.component.scss'
})
export class ReadingGoalsWidgetComponent {
 @Input() goals: UserReadingGoal[] = [];
  @Input() loading = false;
  @Input() showCreateButton = true;

  @Output() createGoal = new EventEmitter<void>();
  @Output() updateGoal = new EventEmitter<UserReadingGoal>();
  @Output() deleteGoal = new EventEmitter<string>();

  get activeGoals(): UserReadingGoal[] {
    return this.goals.filter(g => g.isActive && !g.isCompleted);
  }

  get completedGoals(): UserReadingGoal[] {
    return this.goals.filter(g => g.isCompleted);
  }

  onToggleGoal(goal: UserReadingGoal): void {
    const updatedGoal = { ...goal, isActive: !goal.isActive };
    this.updateGoal.emit(updatedGoal);
  }

  onDeleteGoal(id: string): void {
    this.deleteGoal.emit(id);
  }
}
