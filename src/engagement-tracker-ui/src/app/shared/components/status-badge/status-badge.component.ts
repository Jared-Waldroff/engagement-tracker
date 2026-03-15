import { Component, Input } from '@angular/core';

@Component({
  standalone: false,
  selector: 'app-status-badge',
  templateUrl: './status-badge.component.html',
  styleUrls: ['./status-badge.component.scss']
})
export class StatusBadgeComponent {
  @Input() status = '';

  get colorClass(): string {
    switch (this.status) {
      case 'Active': return 'badge-active';
      case 'Planning': return 'badge-planning';
      case 'OnHold': return 'badge-onhold';
      case 'Completed': return 'badge-completed';
      case 'Cancelled': return 'badge-cancelled';
      case 'OnTrack': return 'badge-ontrack';
      case 'AtRisk': return 'badge-atrisk';
      case 'OverBudget': return 'badge-overbudget';
      default: return 'badge-planning';
    }
  }
}
