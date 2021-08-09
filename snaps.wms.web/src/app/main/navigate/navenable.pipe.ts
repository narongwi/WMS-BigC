import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'navenable'
})
export class NavenablePipe implements PipeTransform {
  transform(objects: any[]): any[] {
    if (objects) {
      return objects.filter(object => {
        return object.isenable === 1;
      });
    }
  }
}
