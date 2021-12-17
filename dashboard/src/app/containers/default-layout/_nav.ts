import { INavData } from '@coreui/angular';

export const navItems: INavData[] = [
  {
    name: 'Devices',
    url: '/devices',
    iconComponent: { name: 'cil-layers' }
  },
  {
    name: 'Logs',
    url: '/logs',
    iconComponent: { name: 'cil-notes' }
  }
];
