import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EngagementSummary } from '../../../../core/models';

@Component({
  standalone: false,
  selector: 'app-top-engagements-table',
  templateUrl: './top-engagements-table.component.html',
  styleUrls: ['./top-engagements-table.component.scss']
})
export class TopEngagementsTableComponent {
  @Input() engagements: EngagementSummary[] = [];
  @Output() rowClick = new EventEmitter<EngagementSummary>();

  displayedColumns = ['clientName', 'title', 'status', 'utilization'];
}
