import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-budget-donut',
  templateUrl: './budget-donut.component.html',
  styleUrls: ['./budget-donut.component.scss']
})
export class BudgetDonutComponent {
  @Input() onTrack = 0;
  @Input() atRisk = 0;
  @Input() overBudget = 0;

  get total(): number {
    return this.onTrack + this.atRisk + this.overBudget;
  }

  get segments(): string {
    if (this.total === 0) return 'conic-gradient(#eee 0deg 360deg)';
    const onTrackDeg = (this.onTrack / this.total) * 360;
    const atRiskDeg = (this.atRisk / this.total) * 360;
    return `conic-gradient(
      #27ae60 0deg ${onTrackDeg}deg,
      #f39c12 ${onTrackDeg}deg ${onTrackDeg + atRiskDeg}deg,
      #c0392b ${onTrackDeg + atRiskDeg}deg 360deg
    )`;
  }
}
