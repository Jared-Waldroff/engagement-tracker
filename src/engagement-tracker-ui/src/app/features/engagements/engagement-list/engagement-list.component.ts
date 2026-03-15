import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/services/auth.service';
import { EngagementSummary, EngagementStatus } from '../../../core/models';
import { EngagementFormDialogComponent } from '../engagement-form-dialog/engagement-form-dialog.component';

@Component({
  standalone: false,
  selector: 'app-engagement-list',
  templateUrl: './engagement-list.component.html',
  styleUrls: ['./engagement-list.component.scss']
})
export class EngagementListComponent implements OnInit, OnDestroy {
  engagements$!: typeof this.engagementService.engagements$;
  loading$!: typeof this.engagementService.loading$;
  error: string | null = null;

  statusFilter: EngagementStatus | '' = '';
  searchQuery = '';
  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  displayedColumns = ['clientName', 'title', 'status', 'utilization', 'managerName', 'startDate'];

  statuses: EngagementStatus[] = ['Planning', 'Active', 'OnHold', 'Completed', 'Cancelled'];

  get canCreate(): boolean {
    return this.authService.hasRole('Manager') || this.authService.hasRole('Partner');
  }

  constructor(
    private engagementService: EngagementService,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.engagements$ = this.engagementService.engagements$;
    this.loading$ = this.engagementService.loading$;
  }

  ngOnInit(): void {
    this.loadData();
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(search => {
      this.searchQuery = search;
      this.loadData();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  onSearchChange(value: string): void {
    this.searchSubject.next(value);
  }

  onStatusChange(): void {
    this.loadData();
  }

  onRowClick(engagement: EngagementSummary): void {
    this.router.navigate(['/engagements', engagement.id]);
  }

  openCreateDialog(): void {
    const dialogRef = this.dialog.open(EngagementFormDialogComponent, {
      width: '600px',
      data: { mode: 'create', clients: this.getUniqueClients() }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadData();
    });
  }

  onRetry(): void {
    this.loadData();
  }

  private loadData(): void {
    this.error = null;
    this.engagementService.loadEngagements({
      status: this.statusFilter,
      search: this.searchQuery
    });
  }

  private getUniqueClients(): { id: number; name: string }[] {
    return [
      { id: 1, name: 'Apex Energy' },
      { id: 2, name: 'Northern Trust Financial' },
      { id: 3, name: 'MedLine Health Systems' },
      { id: 4, name: 'Pinnacle Technologies' },
      { id: 5, name: 'Harvest Retail Group' }
    ];
  }
}
