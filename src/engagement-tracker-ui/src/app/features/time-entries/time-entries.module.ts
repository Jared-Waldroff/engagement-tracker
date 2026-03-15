import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatSnackBarModule } from '@angular/material/snack-bar';

import { SharedModule } from '../../shared/shared.module';
import { TimeEntriesRoutingModule } from './time-entries-routing.module';
import { TimeEntryListComponent } from './time-entry-list/time-entry-list.component';
import { LogTimeDialogComponent } from './log-time-dialog/log-time-dialog.component';

@NgModule({
  declarations: [TimeEntryListComponent, LogTimeDialogComponent],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TimeEntriesRoutingModule,
    SharedModule,
    MatTableModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSnackBarModule
  ]
})
export class TimeEntriesModule {}
