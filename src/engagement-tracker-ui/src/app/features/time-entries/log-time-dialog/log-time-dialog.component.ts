import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TimeEntryService } from '../../../core/services/time-entry.service';
import { EngagementService } from '../../../core/services/engagement.service';
import { EngagementSummary } from '../../../core/models';

export interface LogTimeData {
  engagementId?: number;
  engagementTitle?: string;
}

@Component({
  standalone: false,
  selector: 'app-log-time-dialog',
  templateUrl: './log-time-dialog.component.html',
  styleUrls: ['./log-time-dialog.component.scss']
})
export class LogTimeDialogComponent implements OnInit {
  form: FormGroup;
  isSubmitting = false;
  engagements: EngagementSummary[] = [];

  get hasPreselected(): boolean {
    return !!this.data?.engagementId;
  }

  constructor(
    private fb: FormBuilder,
    private timeEntryService: TimeEntryService,
    private engagementService: EngagementService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<LogTimeDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: LogTimeData
  ) {
    this.form = this.fb.group({
      engagementId: [data?.engagementId || '', Validators.required],
      hours: ['', [Validators.required, Validators.min(0.25), Validators.max(24)]],
      date: [new Date(), Validators.required],
      description: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    if (!this.hasPreselected) {
      this.engagementService.engagements$.subscribe(e => this.engagements = e);
      this.engagementService.loadEngagements();
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSubmitting = true;
    const value = this.form.value;

    this.timeEntryService.createTimeEntry({
      engagementId: value.engagementId,
      hours: value.hours,
      date: value.date instanceof Date ? value.date.toISOString().split('T')[0] : value.date,
      description: value.description
    }).subscribe({
      next: () => {
        this.snackBar.open('Time entry logged.', 'OK', { duration: 3000, panelClass: ['success-snackbar'] });
        this.dialogRef.close(true);
      },
      error: () => { this.isSubmitting = false; }
    });
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
