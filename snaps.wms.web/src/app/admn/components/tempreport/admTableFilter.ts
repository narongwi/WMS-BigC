import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
    name: 'admTableFilter'
})
export class admTableFilter implements PipeTransform {
    // input model name = searchText
    // *ngFor="let item of stores | tableFilter : {storeCode:searchText,storeName:searchText};"
    transform(items: any, filter: any, defaultFilter: boolean = false): any {
        if (!filter) {
            return items;
        }

        if (!Array.isArray(items)) {
            return items;
        }

        if (filter && Array.isArray(items)) {
            let filterKeys = Object.keys(filter);

            if (defaultFilter) {
                return items.filter(item =>
                    filterKeys.reduce((x, keyName) => (x && new RegExp(filter[keyName], 'gi').test(item[keyName])) || filter[keyName] == "", true));
            }
            else {
                return items.filter(item => {
                    return filterKeys.some((keyName) => {
                        return new RegExp(filter[keyName], 'gi').test(item[keyName]) || filter[keyName] == "";
                    });
                });
            }
        }
    }
}
