import { uniqueId } from "lodash";
import {
  IconLayoutDashboard,
  IconTable,
  IconSettings,
  IconCalendarWeek,
  IconChartHistogram
} from "@tabler/icons-react";


const Menuitems = [
  {
    navlabel: true,
    subheader: "Home",
  },
  {
    id: uniqueId(),
    title: "Dashboard",
    icon: IconLayoutDashboard,
    href: "/dashboard",
  },
  {
    id: uniqueId(),
    title: "Tables",
    icon: IconTable,
    href: "/tables",
  },
  {
    id: uniqueId(),
    title: 'Habits',
    icon: IconCalendarWeek,
    href: '/habits',
  },  
  {
    id: uniqueId(),
    title: "Settings",
    icon: IconSettings,
    href: "/settings",
  },
  {
    id: uniqueId(),
    title: "Stats",
    icon: IconChartHistogram,
    href: "/stats",
  }
];

export default Menuitems;
