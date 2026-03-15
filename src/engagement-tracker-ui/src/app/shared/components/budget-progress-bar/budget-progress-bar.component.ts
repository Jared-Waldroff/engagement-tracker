import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-budget-progress-bar',
  templateUrl: './budget-progress-bar.component.html',
  styleUrls: ['./budget-progress-bar.component.scss']
})
export class BudgetProgressBarComponent {
  @Input() utilization = 0;

  get colorClass(): string {
    if (this.utilization >= 100) return 'bar-danger';
    if (this.utilization >= 80) return 'bar-warning';
    return 'bar-success';
  }

  get clampedWidth(): number {
    return Math.min(this.utilization, 100);
  }
}
