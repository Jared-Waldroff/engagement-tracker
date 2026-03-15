import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { EngagementService } from '../../../core/services/engagement.service';
import { AuthService } from '../../../core/services/auth.service';
import { EngagementDetail } from '../../../core/models';
import { EngagementFormDialogComponent } from '../engagement-form-dialog/engagement-form-dialog.component';
import { LogTimeDialogComponent } from '../../time-entries/log-time-dialog/log-time-dialog.component';

@Component({
  standalone: false,
  selector: 'app-engagement-detail',
  templateUrl: './engagement-detail.component.html',
  styleUrls: ['./engagement-detail.component.scss']
})
export class EngagementDetailComponent implements OnInit {
  engagement: EngagementDetail | null = null;
  isLoading = true;
  error: string | null = null;

  userColumns = ['userName', 'role', 'totalHours'];
  timeColumns = ['date', 'userName', 'hours', 'description'];

  get canEdit(): boolean {
    return this.authService.hasRole('Manager') || this.authService.hasRole('Partner');
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private engagementService: EngagementService,
    private authService: AuthService,
    private dialog: MatDialog,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail(id);
  }

  openEditDialog(): void {
    if (!this.engagement) return;
    const dialogRef = this.dialog.open(EngagementFormDialogComponent, {
      width: '600px',
      data: { mode: 'edit', engagement: this.engagement }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDetail(this.engagement!.id);
    });
  }

  openLogTimeDialog(): void {
    if (!this.engagement) return;
    const dialogRef = this.dialog.open(LogTimeDialogComponent, {
      width: '500px',
      data: { engagementId: this.engagement.id, engagementTitle: this.engagement.title }
    });
    dialogRef.afterClosed().subscribe(result => {
      if (result) this.loadDetail(this.engagement!.id);
    });
  }

  onRetry(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail(id);
  }

  goBack(): void {
    this.router.navigate(['/engagements']);
  }

  private loadDetail(id: number): void {
    this.isLoading = true;
    this.error = null;
    this.engagementService.getEngagementDetail(id).subscribe({
      next: data => {
        this.engagement = data;
        this.isLoading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Failed to load engagement details.';
        this.isLoading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
