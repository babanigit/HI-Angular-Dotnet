import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-parent',
  templateUrl: './parent.component.html',
  styleUrls: ['./parent.component.css'],
})
export class ParentComponent implements OnInit {
  constructor() {}

  username: string = '';

  message: string = '';
  messageThroughFun: string = '';

  ngOnInit(): void {}

  handleChildValue(value: string) {
    this.message = value;
  }
}
