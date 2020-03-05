import ErrorBoundary from '@common/components/ErrorBoundary';
import WarningMessage from '@common/components/WarningMessage';
import cartesian from '@common/lib/utils/cartesian';
import formatPretty from '@common/lib/utils/number/formatPretty';
import {
  CategoryFilter,
  Filter,
  Indicator,
  LocationFilter,
  TimePeriodFilter,
} from '@common/modules/table-tool/types/filters';
import { FullTable } from '@common/modules/table-tool/types/fullTable';
import { TableHeadersConfig } from '@common/modules/table-tool/utils/tableHeaders';
import {
  HeaderGroup,
  HeaderSubGroup,
} from '@common/modules/table-tool/components/MultiHeaderTable';
import camelCase from 'lodash/camelCase';
import last from 'lodash/last';
import React, { forwardRef, memo } from 'react';
import DataTableCaption from './DataTableCaption';
import FixedMultiHeaderDataTable from './FixedMultiHeaderDataTable';

interface Props {
  captionTitle?: string;
  fullTable: FullTable;
  tableHeadersConfig: TableHeadersConfig;
}

const createFilterGroupHeaders = (group: Filter[]): HeaderSubGroup[] =>
  group.reduce<HeaderSubGroup[]>((acc, filter) => {
    if (!filter.filterGroup) {
      return acc;
    }

    const lastFilterGroupHeader = last(acc);

    if (!lastFilterGroupHeader) {
      acc.push({
        text: filter.filterGroup,
      });

      return acc;
    }

    if (lastFilterGroupHeader.text === filter.filterGroup) {
      lastFilterGroupHeader.span =
        typeof lastFilterGroupHeader.span !== 'undefined'
          ? lastFilterGroupHeader.span + 1
          : 2;
    } else {
      acc.push({
        text: filter.filterGroup,
      });
    }

    return acc;
  }, []);

export const createGroupHeaders = (groups: Filter[][]): HeaderGroup[] => {
  return groups.reduce((acc, group) => {
    const filterGroupHeaders = createFilterGroupHeaders(group);

    const groupHeaders = group.map(filter => {
      return {
        text: filter.label,
      };
    });

    const hasHeaderFilterGroup =
      filterGroupHeaders.length > 1 &&
      filterGroupHeaders.some(header => header.text !== 'Default');

    acc.push(
      hasHeaderFilterGroup
        ? {
            groups: filterGroupHeaders,
            headers: groupHeaders,
          }
        : {
            headers: groupHeaders,
          },
    );

    return acc;
  }, [] as HeaderGroup[]);
};

const TimePeriodDataTable = forwardRef<HTMLElement, Props>(
  ({ fullTable, tableHeadersConfig, captionTitle }: Props, dataTableRef) => {
    const { subjectMeta, results } = fullTable;

    if (results.length === 0) {
      return (
        <WarningMessage>
          A table could not be returned. There is no data for the options
          selected.
        </WarningMessage>
      );
    }

    const columnHeaders: HeaderGroup[] =
      tableHeadersConfig.columns.length > 1
        ? [
            ...createGroupHeaders(tableHeadersConfig.columnGroups),
            {
              headers: tableHeadersConfig.columns.map(column => ({
                text: column.label,
              })),
            },
          ]
        : createGroupHeaders(tableHeadersConfig.columnGroups);

    const rowHeaders: HeaderGroup[] =
      tableHeadersConfig.rows.length > 1
        ? [
            ...createGroupHeaders(tableHeadersConfig.rowGroups),
            {
              headers: tableHeadersConfig.rows.map(row => ({
                text: row.label,
              })),
            },
          ]
        : createGroupHeaders(tableHeadersConfig.rowGroups);

    const rowHeadersCartesian = cartesian(
      ...tableHeadersConfig.rowGroups,
      tableHeadersConfig.rows as Filter[],
    );

    const columnHeadersCartesian = cartesian(
      ...tableHeadersConfig.columnGroups,
      tableHeadersConfig.columns as Filter[],
    );

    const rows = rowHeadersCartesian.map(rowFilterCombination => {
      const rowCol1 = last(rowFilterCombination);

      return columnHeadersCartesian.map(columnFilterCombination => {
        const rowCol2 = last(columnFilterCombination);

        // User could choose to flip rows and columns
        const indicator = (rowCol1 instanceof Indicator
          ? rowCol1
          : rowCol2) as Indicator;

        const timePeriod = (rowCol2 instanceof TimePeriodFilter
          ? rowCol2
          : rowCol1) as TimePeriodFilter;

        const filterCombination = [
          ...rowFilterCombination,
          ...columnFilterCombination,
        ];

        const categoryFilters = filterCombination.filter(
          filter => filter instanceof CategoryFilter,
        );

        const locationFilters = filterCombination.filter(
          filter => filter instanceof LocationFilter,
        ) as LocationFilter[];

        const matchingResult = results.find(result => {
          return (
            categoryFilters.every(filter =>
              result.filters.includes(filter.value),
            ) &&
            result.timePeriod === timePeriod.value &&
            locationFilters.every(filter => {
              const geographicLevel = camelCase(result.geographicLevel);
              return (
                result.location[geographicLevel] &&
                result.location[geographicLevel].code === filter.value &&
                filter.level === geographicLevel
              );
            })
          );
        });

        if (!matchingResult) {
          return 'n/a';
        }

        const value = matchingResult.measures[indicator.value];

        if (Number.isNaN(Number(value))) {
          return value;
        }

        return indicator.unit === '£'
          ? `${indicator.unit}${formatPretty(value)}`
          : `${formatPretty(value)}${indicator.unit}`;
      });
    });

    return (
      <ErrorBoundary
        fallback={
          <WarningMessage>
            There was a problem rendering your table.
          </WarningMessage>
        }
      >
        <FixedMultiHeaderDataTable
          caption={
            <DataTableCaption
              {...subjectMeta}
              title={captionTitle}
              id="dataTableCaption"
            />
          }
          columnHeaders={columnHeaders}
          rowHeaders={rowHeaders}
          rows={rows}
          ref={dataTableRef}
          footnotes={subjectMeta.footnotes}
        />
      </ErrorBoundary>
    );
  },
);

TimePeriodDataTable.displayName = 'TimePeriodDataTable';

export default memo(TimePeriodDataTable);
