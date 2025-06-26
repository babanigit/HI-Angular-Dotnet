import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
} from '@angular/core';

@Component({
  selector: 'app-child',
  templateUrl: './child.component.html',
  styleUrls: ['./child.component.css'],
})
export class ChildComponent implements OnInit, OnChanges {
  @Input() name: string = '';

  @Input() value: string = '';
  @Output() valueChange = new EventEmitter<string>();

  onValueChange(newValue: string) {
    this.value = newValue;
    this.valueChange.emit(newValue); // Emit to parent
  }

  sendToParent() {}

  constructor() {}

  ngOnInit(): void {}
  ngOnChanges(changes: SimpleChanges) {
    console.log('Changes in child components:', changes);
  }
}
