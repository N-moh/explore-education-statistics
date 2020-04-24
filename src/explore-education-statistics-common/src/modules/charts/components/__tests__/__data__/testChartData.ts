import { TableDataResponse } from '@common/services/tableBuilderService';
import { Chart } from '@common/services/types/blocks';

export const testChartConfiguration: Chart = {
  legend: 'top',
  axes: {
    major: {
      type: 'major',
      groupBy: 'timePeriod',
      sortAsc: true,
      referenceLines: [],
      dataSets: [
        {
          indicator: 'unauthorised-absence-rate',
          filters: ['characteristic-total', 'school-type-total'],
          config: {
            label: 'Unauthorised absence rate',
            unit: '%',
            colour: '#4763a5',
            symbol: 'circle',
            lineStyle: 'solid',
          },
        },
        {
          indicator: 'overall-absence-rate',
          filters: ['characteristic-total', 'school-type-total'],
          config: {
            label: 'Overall absence rate',
            unit: '%',
            colour: '#f5a450',
            symbol: 'cross',
            lineStyle: 'solid',
          },
        },
        {
          indicator: 'authorised-absence-rate',
          filters: ['characteristic-total', 'school-type-total'],
          config: {
            label: 'Authorised absence rate',
            unit: '%',
            colour: '#005ea5',
            symbol: 'diamond',
            lineStyle: 'solid',
          },
        },
      ],
      visible: true,
      title: 'School Year',
      size: 50,
      showGrid: true,
      labelPosition: 'axis',
      tickConfig: 'default',
    },
    minor: {
      type: 'major',
      groupBy: 'timePeriod',
      referenceLines: [],
      dataSets: [],
      sortAsc: true,
      visible: true,
      title: 'Absence Rate',
      size: 50,
      showGrid: true,
      labelPosition: 'axis',
      min: 0,
      tickConfig: 'default',
    },
  },
  type: 'line',
  title: 'Aggregated results chart',
  height: 300,
  width: 600,
};

/**
 * @deprecated
 */
export const testDeprecatedChartConfiguration: Chart = {
  legend: 'top',
  axes: {
    major: {
      type: 'major',
      groupBy: 'timePeriod',
      sortAsc: true,
      referenceLines: [],
      dataSets: [
        {
          indicator: 'unauthorised-absence-rate',
          filters: ['characteristic-total', 'school-type-total'],
        },
        {
          indicator: 'overall-absence-rate',
          filters: ['characteristic-total', 'school-type-total'],
        },
        {
          indicator: 'authorised-absence-rate',
          filters: ['characteristic-total', 'school-type-total'],
        },
      ],
      visible: true,
      title: 'School Year',
      size: 50,
      showGrid: true,
      labelPosition: 'axis',
      tickConfig: 'default',
    },
    minor: {
      type: 'major',
      groupBy: 'timePeriod',
      referenceLines: [],
      dataSets: [],
      sortAsc: true,
      visible: true,
      title: 'Absence Rate',
      size: 50,
      showGrid: true,
      labelPosition: 'axis',
      min: 0,
      tickConfig: 'default',
    },
  },
  labels: {
    'unauthorised-absence-rate_characteristic-total_school-type-total_____': {
      value: 'unauthorised-absence-rate_characteristic-total_school-type-total',
      label: 'Unauthorised absence rate',
      unit: '%',
      colour: '#4763a5',
      symbol: 'circle',
      lineStyle: 'solid',
    },
    'overall-absence-rate_characteristic-total_school-type-total_____': {
      value: 'overall-absence-rate_characteristic-total_school-type-total',
      label: 'Overall absence rate',
      unit: '%',
      colour: '#f5a450',
      symbol: 'cross',
      lineStyle: 'solid',
    },
    'authorised-absence-rate_characteristic-total_school-type-total_____': {
      value: 'authorised-absence-rate_characteristic-total_school-type-total',
      label: 'Authorised absence rate',
      unit: '%',
      colour: '#005ea5',
      symbol: 'diamond',
      lineStyle: 'solid',
    },
  },
  type: 'line',
  title: 'Aggregated results chart',
  height: 300,
  width: 600,
};

export const testChartTableData: TableDataResponse = {
  subjectMeta: {
    filters: {
      SchoolType: {
        totalValue: 'school-type-total',
        hint: 'Filter by school type',
        legend: 'School type',
        options: {
          Default: {
            label: 'Default',
            options: [{ label: 'Total', value: 'school-type-total' }],
          },
        },
        name: 'school_type',
      },
      Characteristic: {
        totalValue: 'characteristic-total',
        hint: 'Filter by pupil characteristic',
        legend: 'Characteristic',
        options: {
          Total: {
            label: 'Total',
            options: [{ label: 'Total', value: 'characteristic-total' }],
          },
        },
        name: 'characteristic',
      },
    },
    footnotes: [],
    indicators: [
      {
        label: 'Authorised absence rate',
        unit: '%',
        value: 'authorised-absence-rate',
        name: 'sess_authorised_percent',
      },
      {
        label: 'Unauthorised absence rate',
        unit: '%',
        value: 'unauthorised-absence-rate',
        name: 'sess_unauthorised_percent',
      },
      {
        label: 'Overall absence rate',
        unit: '%',
        value: 'overall-absence-rate',
        name: 'sess_overall_percent',
      },
    ],
    locations: [{ level: 'country', label: 'England', value: 'E92000001' }],
    boundaryLevels: [
      {
        id: 1,
        label:
          'Countries December 2017 Ultra Generalised Clipped Boundaries in UK',
      },
    ],
    publicationName: 'Pupil absence in schools in England',
    subjectName: 'Absence by characteristic',
    timePeriodRange: [
      { code: 'AY', label: '2012/13', year: 2012 },
      { code: 'AY', label: '2013/14', year: 2013 },
      { code: 'AY', label: '2014/15', year: 2014 },
      { code: 'AY', label: '2015/16', year: 2015 },
      { code: 'AY', label: '2016/17', year: 2016 },
    ],
    geoJsonAvailable: true,
  },
  results: [
    {
      filters: ['characteristic-total', 'school-type-total'],
      geographicLevel: 'Country',
      location: { country: { code: 'E92000001', name: 'England' } },
      measures: {
        'authorised-absence-rate': '3.5',
        'overall-absence-rate': '4.6',
        'unauthorised-absence-rate': '1.1',
      },
      timePeriod: '2014_AY',
    },
    {
      filters: ['characteristic-total', 'school-type-total'],
      geographicLevel: 'Country',
      location: { country: { code: 'E92000001', name: 'England' } },
      measures: {
        'authorised-absence-rate': '4.2',
        'overall-absence-rate': '5.3',
        'unauthorised-absence-rate': '1.1',
      },
      timePeriod: '2012_AY',
    },
    {
      filters: ['characteristic-total', 'school-type-total'],
      geographicLevel: 'Country',
      location: { country: { code: 'E92000001', name: 'England' } },
      measures: {
        'authorised-absence-rate': '3.5',
        'overall-absence-rate': '4.5',
        'unauthorised-absence-rate': '1.1',
      },
      timePeriod: '2013_AY',
    },
    {
      filters: ['characteristic-total', 'school-type-total'],
      geographicLevel: 'Country',
      location: { country: { code: 'E92000001', name: 'England' } },
      measures: {
        'authorised-absence-rate': '3.4',
        'overall-absence-rate': '4.7',
        'unauthorised-absence-rate': '1.3',
      },
      timePeriod: '2016_AY',
    },
    {
      filters: ['characteristic-total', 'school-type-total'],
      geographicLevel: 'Country',
      location: { country: { code: 'E92000001', name: 'England' } },
      measures: {
        'authorised-absence-rate': '3.4',
        'overall-absence-rate': '4.6',
        'unauthorised-absence-rate': '1.1',
      },
      timePeriod: '2015_AY',
    },
  ],
};