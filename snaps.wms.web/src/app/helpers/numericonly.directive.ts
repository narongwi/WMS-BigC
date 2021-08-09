import { Directive, ElementRef, HostListener, Input } from '@angular/core';
import { NgModel } from '@angular/forms';

@Directive({ selector: '[appOnlynumber]' })
export class OnlynumberDirective {
  
  private navigationKeys = [
    'Backspace',
    'Delete',
    'Tab',
    'Escape',
    'Enter',
    'Home',
    'End',
    'ArrowLeft',
    'ArrowRight',
    'Clear',
    'Copy',
    'Paste'
  ];
  inputElement: HTMLElement;
  inputNative: HTMLInputElement;
  @Input('allowDigit') allowDigit :boolean = false;
  @Input('allowMinus') allowMinus :boolean = false;
  constructor(public el: ElementRef, public model:NgModel) {
    this.inputElement = el.nativeElement;
    this.inputNative = el.nativeElement;
  }

  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    if (
      this.navigationKeys.indexOf(e.key) > -1 || // Allow: navigation keys: backspace, delete, arrows etc.
      (e.key === 'a' && e.ctrlKey === true) || // Allow: Ctrl+A
      (e.key === 'c' && e.ctrlKey === true) || // Allow: Ctrl+C
      (e.key === 'v' && e.ctrlKey === true) || // Allow: Ctrl+V
      (e.key === 'x' && e.ctrlKey === true) || // Allow: Ctrl+X
      (e.key === 'a' && e.metaKey === true) || // Allow: Cmd+A (Mac)
      (e.key === 'c' && e.metaKey === true) || // Allow: Cmd+C (Mac)
      (e.key === 'v' && e.metaKey === true) || // Allow: Cmd+V (Mac)
      (e.key === 'x' && e.metaKey === true) ||// Allow: Cmd+X (Mac)
      (e.key === "." && this.allowDigit == true ) || // Allow to use digit
      (e.key === "-" && this.allowMinus == true ) // Allow to use digit
    ) {
      // let it happen, don't do anything
      return;
    }
    // Ensure that it is a number and stop the keypress
    if (e.key === ' ' || isNaN(Number(e.key))) {
      e.preventDefault();
    }

  }

  @HostListener('keyup', ['$event'])
  onKeyup(e: KeyboardEvent) {
    if (parseInt(this.inputElement.getAttribute("max"))) {
      //console.log("validate max ");
      if (parseInt(this.inputNative.value) > parseInt(this.inputElement.getAttribute("max"))) {
        //console.log("setup new value ");
        this.inputNative.value = this.inputElement.getAttribute("max");
        this.model.update.emit(this.inputNative.value); //update value on model
        //this.model.valueAccessor.writeValue(this.inputNative.value);
      }
    }

  }

  @HostListener('paste', ['$event'])
  public onPaste(event: ClipboardEvent) {
    event.preventDefault();
    const pastedInput: string = event.clipboardData
      .getData('text/plain')
      .replace(/\D/g, ''); // get a digit-only string
    document.execCommand('insertText', false, pastedInput);
  }

  @HostListener('drop', ['$event'])
  onDrop(event: DragEvent) {
    event.preventDefault();
    const textData = event.dataTransfer.getData('text').replace(/\D/g, '');
    this.inputElement.focus();
    document.execCommand('insertText', false, textData);
  }


}
