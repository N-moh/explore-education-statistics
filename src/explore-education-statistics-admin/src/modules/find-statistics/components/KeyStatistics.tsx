import { ErrorControlContext } from '@admin/components/ErrorBoundary';
import { EditableContentBlock } from '@admin/services/publicationService';
import { releaseContentService } from '@admin/services/release/edit-release/content/service';
import Button from '@common/components/Button';
import styles from '@common/modules/find-statistics/components/SummaryRenderer.module.scss';
import { EditingContext } from '@common/modules/find-statistics/util/wrapEditableComponent';
import {
  AbstractRelease,
  Publication,
} from '@common/services/publicationService';
import React, { useContext, useEffect, useState } from 'react';
import EditableKeyStatTile from './EditableKeyStatTile';
import KeyIndicatorSelectForm from './KeyIndicatorSelectForm';

export interface KeyStatisticsProps {
  release: AbstractRelease<EditableContentBlock, Publication>;
  setRelease: (
    newRelease: AbstractRelease<EditableContentBlock, Publication>,
  ) => void;
  isEditing?: boolean;
}

const KeyStatistics = ({
  release,
  setRelease,
  isEditing,
}: KeyStatisticsProps) => {
  const { updateAvailableDataBlocks } = useContext(EditingContext);
  const { handleApiErrors } = useContext(ErrorControlContext);
  const [keyStats, setKeyStats] = useState<
    AbstractRelease<EditableContentBlock, Publication>['keyStatisticsSection']
  >();

  useEffect(() => {
    if (release.keyStatisticsSection) {
      setKeyStats(release.keyStatisticsSection);
    } else {
      setKeyStats(undefined);
    }
  }, [release]);

  if (!keyStats) return null;
  return (
    <>
      {isEditing && (
        <AddKeyStatistics release={release} setRelease={setRelease} />
      )}
      <div className={styles.keyStatsContainer}>
        {keyStats.content &&
          keyStats.content.map(stat => {
            if (stat.dataBlockRequest !== undefined) {
              return stat.type === 'DataBlock' && stat.dataBlockRequest ? (
                // @ts-ignore
                <EditableKeyStatTile
                  key={stat.id}
                  {...stat}
                  isEditing={isEditing}
                  onRemove={() => {
                    releaseContentService
                      .deleteContentSectionBlock(
                        release.id,
                        release.keyStatisticsSection.id || '',
                        stat.id,
                      )
                      .then(() => {
                        setRelease({
                          ...release,
                          keyStatisticsSection: {
                            ...release.keyStatisticsSection,
                            content:
                              release.keyStatisticsSection.content &&
                              release.keyStatisticsSection.content.filter(
                                contentBlock => contentBlock.id !== stat.id,
                              ),
                          },
                        });
                        if (updateAvailableDataBlocks) {
                          updateAvailableDataBlocks();
                        }
                      })
                      .catch(handleApiErrors);
                  }}
                  onSubmit={values => {
                    return new Promise(resolve =>
                      releaseContentService
                        .updateContentSectionDataBlock(
                          release.id,
                          release.keyStatisticsSection.id as string,
                          stat.id,
                          values,
                        )
                        .then(updatedBlock => {
                          setRelease({
                            ...release,
                            keyStatisticsSection: {
                              ...release.keyStatisticsSection,
                              content: release.keyStatisticsSection.content
                                ? release.keyStatisticsSection.content.map(
                                    contentBlock => {
                                      if (contentBlock.id === updatedBlock.id) {
                                        return updatedBlock;
                                      }
                                      return contentBlock;
                                    },
                                  )
                                : [],
                            },
                          });
                          resolve();
                        })
                        .catch(handleApiErrors),
                    );
                  }}
                />
              ) : null;
            }
            return null;
          })}
      </div>
    </>
  );
};

const AddKeyStatistics = ({ release, setRelease }: KeyStatisticsProps) => {
  const { handleApiErrors } = useContext(ErrorControlContext);
  const { updateAvailableDataBlocks } = useContext(EditingContext);
  const [isFormOpen, setIsFormOpen] = useState<boolean>(false);

  const another =
    release.keyStatisticsSection.content &&
    release.keyStatisticsSection.content.length > 0 &&
    ' another ';
  return (
    <>
      {isFormOpen && (
        <KeyIndicatorSelectForm
          onSelect={async datablockId => {
            if (
              release.keyStatisticsSection &&
              release.keyStatisticsSection.id
            ) {
              await releaseContentService
                .attachContentSectionBlock(
                  release.id,
                  release.keyStatisticsSection &&
                    release.keyStatisticsSection.id,
                  {
                    contentBlockId: datablockId,
                    order:
                      (release.keyStatisticsSection.content &&
                        release.keyStatisticsSection.content.length) ||
                      0,
                  },
                )
                .then(v => {
                  if (updateAvailableDataBlocks) {
                    updateAvailableDataBlocks();
                  }
                  return v;
                })
                .catch(handleApiErrors);

              const keyStatisticsSection = await releaseContentService.getContentSection(
                release.id,
                release.keyStatisticsSection.id,
              );
              if (keyStatisticsSection) {
                setRelease({
                  ...release,
                  keyStatisticsSection,
                });
                setIsFormOpen(false);
              }
            }
          }}
          onCancel={() => setIsFormOpen(false)}
        />
      )}
      {!isFormOpen && (
        <Button
          onClick={() => {
            setIsFormOpen(true);
          }}
        >
          Add {another} key statistic
        </Button>
      )}
    </>
  );
};

export default KeyStatistics;