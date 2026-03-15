import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TimeEntryService } from '../../../core/services/time-entry.service';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/services/auth.service';
import { EngagementSummary } from '../../../core/models';
import { LogTimeDialogComponent } from '../log-time-dialog/log-time-dialog.component';

@Component({
  standalone: false,
  selector: 'app-time-entry-list',
  templateUrl: './time-entry-list.component.html',
  styleUrls: ['./time-entry-list.component.scss']
})
export class TimeEntryListComponent implements OnInit {
  timeEntries$!: typeof this.timeEntryService.timeEntries$;
  loading$!: typeof this.timeEntryService.loading$;
  engagements: EngagementSummary[] = [];
  selectedEngagementId: number | undefined;

  get displayedColumns(): string[] {
    const cols = ['date', 'engagementTitle'];
    if (this.authService.hasRole('Manager') || this.authService.hasRole('Partner')) {
      cols.push('userName');
    }
    cols.push('hours', 'description');
    return cols;
  }

  constructor(
    private timeEntryService: TimeEntryService,
    private engagementService: EngagementService,
    private authService: AuthService,
    private dialog: MatDialog
  ) {
    this.timeEntries$ = this.timeEntryService.timeEntries$;
    this.loading$ = this.timeEntryService.loading$;
  }

  ngOnInit(): void {
    this.timeEntryService.loadTimeEntries();
    this.engagementService.engagements$.subscribe(e => this.engagements = e);
    this.engagementService.loadEngagements();
  }

  onEngagementFilter(engagementId: number | undefined): void {
    this.selectedEngagementId = engagementId;
    this.timeEntryService.loadTimeEntries(engagementId);
  }

  openLogTimeDialog(): void {
    const dialogRef = this.dialog.open(LogTimeDialogComponent, {
      width: '500px',
      data: {}
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.timeEntryService.loadTimeEntries(this.selectedEngagementId);
    });
  }

  onRetry(): void {
    this.timeEntryService.loadTimeEntries(this.selectedEngagementId);
  }
}
