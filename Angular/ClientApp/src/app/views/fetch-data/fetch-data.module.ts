import { NgModule } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { ButtonsModule } from "ngx-bootstrap/buttons";

import { FetchDataComponent } from "./fetch-data.component";
import { FetchDataRoutingModule } from "./fetch-data-routing.module";
import { CommonModule } from "@angular/common";

@NgModule({
  imports: [
    FormsModule,
    FetchDataRoutingModule,
    CommonModule,
    ButtonsModule.forRoot(),
  ],
  declarations: [FetchDataComponent],
})
export class FetchDataModule { }
