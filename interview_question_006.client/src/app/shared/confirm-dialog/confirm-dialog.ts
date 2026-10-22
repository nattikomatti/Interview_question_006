import { Component, HostListener, input, output } from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  templateUrl: './confirm-dialog.html',
  standalone: false,
  styleUrl: './confirm-dialog.css',
})
export class ConfirmDialog {
  readonly message = input.required<string>();
  readonly confirmText = input('ตกลง');
  readonly cancelText = input('ยกเลิก');

  readonly confirmed = output<void>();
  readonly cancelled = output<void>();

  @HostListener('document:keydown.escape')
  onEscape() {
    this.cancelled.emit();
  }
}
