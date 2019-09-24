import LoadingSpinner from '@common/components/LoadingSpinner';
import Tabs from '@common/components/Tabs';
import TabsSection from '@common/components/TabsSection';
import ChartRenderer, {
  ChartRendererProps,
} from '@common/modules/find-statistics/components/ChartRenderer';

import DataSource from '@common/modules/find-statistics/components/DataSource';
import SummaryRenderer, {
  SummaryRendererProps,
} from '@common/modules/find-statistics/components/SummaryRenderer';
import TableRenderer, {
  Props as TableRendererProps,
} from '@common/modules/find-statistics/components/TableRenderer';
import DataBlockService, {
  DataBlockData,
  DataBlockRequest,
  DataBlockResponse,
} from '@common/services/dataBlockService';
import {
  Chart,
  DataQuery,
  Summary,
  Table,
} from '@common/services/publicationService';
import React, { Component, MouseEvent, ReactNode } from 'react';
import DownloadDetails from './DownloadDetails';

export interface DataBlockProps {
  id: string;
  type: string;
  heading?: string;
  dataBlockRequest?: DataBlockRequest;
  charts?: Chart[];
  tables?: Table[];

  summary?: Summary;

  height?: number;
  showTables?: boolean;
  additionalTabContent?: ReactNode;

  onToggle?: (section: { id: string; title: string }) => void;

  dataBlockResponse?: DataBlockResponse;

  onSummaryDetailsToggle?: (
    isOpened: boolean,
    event: MouseEvent<HTMLElement>,
  ) => void;
}

interface DataBlockState {
  isLoading: boolean;
  isError: boolean;
  charts?: ChartRendererProps[];
  tables?: TableRendererProps[];
  summary?: SummaryRendererProps;
}

class DataBlock extends Component<DataBlockProps, DataBlockState> {
  public static defaultProps = {
    showTables: true,
  };

  public state: DataBlockState = {
    isLoading: false,
    isError: false,
  };

  private query?: DataQuery = undefined;

  public async componentDidMount() {
    const { dataBlockRequest } = this.props;

    this.setState({ isLoading: true, isError: false });

    if (dataBlockRequest) {
      const result = await DataBlockService.getDataBlockForSubject(
        dataBlockRequest,
      );

      if (result) {
        this.parseDataResponse(result);
      } else {
        this.setState({
          isError: true,
          isLoading: false,
        });
      }
    } else {
      const { dataBlockResponse } = this.props;
      if (dataBlockResponse) {
        this.parseDataResponse(dataBlockResponse);
      }
    }
  }

  public async componentWillUnmount() {
    this.query = undefined;
  }

  private parseDataResponse(response: DataBlockResponse): void {
    const newState: DataBlockState = { isLoading: false, isError: false };

    const data: DataBlockData = response;
    const meta = response.metaData;

    const { charts, summary, tables } = this.props;

    if (response.result.length > 0) {
      if (tables) {
        newState.tables = [
          {
            data,
            meta,
            ...tables[0], /// at present only one chart
          },
        ];
      }
    }

    if (charts) {
      newState.charts = charts.map<ChartRendererProps>(chart => {
        // There is a presumption that the configuration from the API is valid.
        // The data coming from the API is required to be optional for the ChartRenderer
        // But the data for the charts is required. The charts have validation that
        // prevent them from attempting to render.
        // @ts-ignore
        const rendererProps: ChartRendererProps = {
          data,
          meta,
          ...chart,
        };

        return rendererProps;
      });
    }

    if (summary) {
      newState.summary = {
        ...summary,
        data,
        meta,
      };
    }
    this.setState(newState);
  }

  public render() {
    const {
      heading = '',
      height,
      showTables,
      additionalTabContent,
      onToggle,
      onSummaryDetailsToggle,
      id,
    } = this.props;
    const { charts, summary, tables, isLoading, isError } = this.state;
    return (
      <>
        {heading && <h3>{heading}</h3>}

        {isLoading ? (
          <LoadingSpinner text="Loading content..." />
        ) : (
          <Tabs id={id} onToggle={onToggle}>
            {isError && (
              <TabsSection id={`${id}-error`} title="Error">
                An error occurred while loading the data, please try again later
              </TabsSection>
            )}

            {summary && (
              <TabsSection id={`${id}-summary`} title="Summary">
                <SummaryRenderer
                  onToggle={onSummaryDetailsToggle}
                  {...summary}
                />
              </TabsSection>
            )}

            {tables && showTables && (
              <TabsSection id={`${id}-tables`} title="Data tables">
                {tables.map((table, idx) => {
                  const key = `${id}0_table_${idx}`;

                  return (
                    <React.Fragment key={key}>
                      <TableRenderer {...table} />
                      <DataSource />
                      <DownloadDetails />
                    </React.Fragment>
                  );
                })}

                {additionalTabContent}
              </TabsSection>
            )}

            {charts && (
              <TabsSection id={`${id}-charts`} title="Charts" lazy={false}>
                {charts.map((chart, idx) => {
                  const key = `${id}_chart_${idx}`;

                  return (
                    <React.Fragment key={key}>
                      {chart.data &&
                      chart.meta &&
                      chart.data.result.length > 0 ? (
                        <>
                          <ChartRenderer {...chart} height={height}>
                            <DataSource />
                            <DownloadDetails />
                          </ChartRenderer>
                        </>
                      ) : (
                        <div>
                          Unable to render chart, invalid data configured
                        </div>
                      )}
                    </React.Fragment>
                  );
                })}

                {additionalTabContent}
              </TabsSection>
            )}
          </Tabs>
        )}
      </>
    );
  }
}

export default DataBlock;
