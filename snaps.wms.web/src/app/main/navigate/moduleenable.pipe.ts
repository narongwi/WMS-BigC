import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'moduleenable'
})
export class ModuleenablePipe implements PipeTransform {

  transform(value: unknown, ...args: unknown[]): unknown {
    return null;
  }

}
