import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

    public title: string;

    public progress: any = { ratio: 0};

    private interval: any;

    constructor(private http: Http) {
        this.title = 'Comic Box';        
    }

    ngOnInit() {
        this.interval = setInterval(() => this.callApi(), 3000);
        this.callApi();
    }

    private callApi() {
        this.http.get("api/progress/thumbnailworkerstatus").subscribe(response => {
            this.progress = response.json();
            this.progress.ratio = Math.ceil(this.progress.inProgressCompletedCount / this.progress.inProgressTotalCount * 100)
            if (!this.progress.isInProgress) {
                clearInterval(this.interval);
            }
        });
    }
}

