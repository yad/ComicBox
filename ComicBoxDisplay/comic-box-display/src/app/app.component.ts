import { Component, OnInit } from '@angular/core';
import { Http, Response } from '@angular/http';
import { WorkerService } from './worker.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

    public title: string;

    public progress: any = { ratio: 0};

    private interval: any;

    constructor(private workerService: WorkerService) {
        this.title = 'Comic Box';        
    }

    ngOnInit() {
        this.interval = setInterval(() => this.callApi(), 3000);
        this.callApi();
    }

    private callApi() {
        this.workerService.getThumbnailWorkerStatus().subscribe(result => {
            this.progress = result;
            if (!this.progress.isInProgress) {
                clearInterval(this.interval);
            }
        });
    }
}

