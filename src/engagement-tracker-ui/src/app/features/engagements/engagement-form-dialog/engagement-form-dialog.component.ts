import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { EngagementService } from '../../../core/services/engagement.service';
import { EngagementDetail, EngagementStatus } from '../../../core/models';

export interface EngagementFormData {
  mode: 'create' | 'edit';
  clients?: { id: number; name: string }[];
  engagement?: EngagementDetail;
}

@Component({
  standalone: false,
  selector: 'app-engagement-form-dialog',
  templateUrl: './engagement-form-dialog.component.html',
  styleUrls: ['./engagement-form-dialog.component.scss']
})
export class EngagementFormDialogComponent {
  form: FormGroup;
  isSubmitting = false;
  statuses: EngagementStatus[] = ['Planning', 'Active', 'OnHold', 'Completed', 'Cancelled'];

  get isEdit(): boolean { return this.data.mode === 'edit'; }
  get title(): string { return this.isEdit ? 'Edit Engagement' : 'New Engagement'; }

  constructor(
    private fb: FormBuilder,
    private engagementService: EngagementService,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<EngagementFormDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: EngagementFormData
  ) {
    if (this.isEdit && data.engagement) {
      this.form = this.fb.group({
        title: [data.engagement.title, Validators.required],
        description: [data.engagement.description, Validators.required],
        status: [data.engagement.status, Validators.required],
        budgetHours: [data.engagement.budgetHours, [Validators.required, Validators.min(1)]],
        hourlyRate: [data.engagement.hourlyRate, [Validators.required, Validators.min(1)]],
        endDate: [data.engagement.endDate || '']
      });
    } else {
      this.form = this.fb.group({
        clientId: ['', Validators.required],
        title: ['', Validators.required],
        description: ['', Validators.required],
        budgetHours: ['', [Validators.required, Validators.min(1)]],
        hourlyRate: ['', [Validators.required, Validators.min(1)]],
        startDate: ['', Validators.required],
        endDate: ['']
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const value = this.form.value;

    if (this.isEdit && this.data.engagement) {
      this.engagementService.updateEngagement(this.data.engagement.id, {
        title: value.title,
        description: value.description,
        status: value.status,
        budgetHours: value.budgetHours,
        hourlyRate: value.hourlyRate,
        endDate: value.endDate || undefined
      }).subscribe({
        next: () => {
          this.snackBar.open('Engagement updated.', 'OK', { duration: 3000, panelClass: ['success-snackbar'] });
          this.dialogRef.close(true);
        },
        error: () => { this.isSubmitting = false; }
      });
    } else {
      this.engagementService.createEngagement({
        clientId: value.clientId,
        title: value.title,
        description: value.description,
        budgetHours: value.budgetHours,
        hourlyRate: value.hourlyRate,
        startDate: value.startDate,
        endDate: value.endDate || undefined
      }).subscribe({
        next: () => {
          this.snackBar.open('Engagement created.', 'OK', { duration: 3000, panelClass: ['success-snackbar'] });
          this.dialogRef.close(true);
        },
        error: () => { this.isSubmitting = false; }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
