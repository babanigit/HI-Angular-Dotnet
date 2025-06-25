import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // ✅ Required for animations
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarModule } from './components/navbar/navbar.module';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { ToastrModule } from 'ngx-toastr';
import { FormsModule } from '@angular/forms'; // ✅ Import this
import { AuthInterceptor } from './util/auth.interceptor';

@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NavbarModule,
    HttpClientModule,

    BrowserAnimationsModule, // ✅ Now properly imported
    ToastrModule.forRoot({
      timeOut: 1200,
      positionClass: 'toast-top-right',
      // easeTime: 300,
      preventDuplicates: true,
      progressBar: true,
      closeButton: true,
      tapToDismiss: true,
    }),
    FormsModule, // ✅ Add this
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
