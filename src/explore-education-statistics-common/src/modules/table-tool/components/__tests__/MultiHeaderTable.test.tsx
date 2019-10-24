import React from 'react';
import { render } from 'react-testing-library';
import MultiHeaderTable, { generateHeaderSpanInfo, generateHeaders } from '../MultiHeaderTable';

describe('MultiHeaderTable', () => {
  test('renders 2x2 table correctly', () => {
    const { container } = render(
      <MultiHeaderTable
        columnHeaders={[['A', 'B'], ['C', 'D']]}
        rowHeaders={[['1', '2'], ['3', '4']]}
        rows={[
          ['AC13', 'AD13', 'BC13', 'BD13'],
          ['AC14', 'AD14', 'BC14', 'BD14'],
          ['AC23', 'AD23', 'BC23', 'BC23'],
          ['AC24', 'AD24', 'BC23', 'BC24'],
        ]}
      />,
    );

    expect(container.querySelectorAll('thead tr')).toHaveLength(2);
    expect(
      container.querySelectorAll('thead tr:nth-child(1) th[scope="colgroup"]'),
    ).toHaveLength(2);
    expect(
      container.querySelectorAll('thead tr:nth-child(2) th[scope="col"]'),
    ).toHaveLength(4);

    expect(container.querySelectorAll('tbody tr')).toHaveLength(4);
    expect(container.querySelectorAll('tbody tr:nth-child(1) td')).toHaveLength(
      4,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(2) td')).toHaveLength(
      4,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(3) td')).toHaveLength(
      4,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(4) td')).toHaveLength(
      4,
    );

    expect(container.innerHTML).toMatchSnapshot();
  });

  test('renders 2x2x2 table correctly', () => {
    const { container } = render(
      <MultiHeaderTable
        columnHeaders={[['A', 'B'], ['C', 'D'], ['E', 'F']]}
        rowHeaders={[['1', '2'], ['3', '4'], ['5', '6']]}
        rows={[
          [
            'ACE135',
            'ACF135',
            'ADE135',
            'ADF135',
            'BCE135',
            'BCF135',
            'BDE135',
            'BDF135',
          ],
          [
            'ACE136',
            'ACF136',
            'ADE136',
            'ADF136',
            'BCE136',
            'BCF136',
            'BDE136',
            'BDF136',
          ],
          [
            'ACE145',
            'ACF145',
            'ADE145',
            'ADF145',
            'BCE145',
            'BCF145',
            'BDE145',
            'BDF145',
          ],
          [
            'ACE146',
            'ACF146',
            'ADE146',
            'ADF146',
            'BCE146',
            'BCF146',
            'BDE146',
            'BDF146',
          ],
          [
            'ACE235',
            'ACF235',
            'ADE235',
            'ADF235',
            'BCE235',
            'BCF235',
            'BDE235',
            'BDF235',
          ],
          [
            'ACE236',
            'ACF236',
            'ADE236',
            'ADF236',
            'BCE236',
            'BCF236',
            'BDE236',
            'BDF236',
          ],
          [
            'ACE245',
            'ACF245',
            'ADE245',
            'ADF245',
            'BCE245',
            'BCF245',
            'BDE245',
            'BDF245',
          ],
          [
            'ACE246',
            'ACF246',
            'ADE246',
            'ADF246',
            'BCE246',
            'BCF246',
            'BDE246',
            'BDF246',
          ],
        ]}
      />,
    );

    expect(container.querySelectorAll('thead tr')).toHaveLength(3);
    expect(
      container.querySelectorAll('thead tr:nth-child(1) th[scope="colgroup"]'),
    ).toHaveLength(2);
    expect(
      container.querySelectorAll('thead tr:nth-child(2) th[scope="colgroup"]'),
    ).toHaveLength(4);
    expect(
      container.querySelectorAll('thead tr:nth-child(3) th[scope="col"]'),
    ).toHaveLength(8);

    expect(container.querySelectorAll('tbody tr')).toHaveLength(8);
    expect(container.querySelectorAll('tbody tr:nth-child(1) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(2) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(3) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(4) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(5) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(6) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(7) td')).toHaveLength(
      8,
    );
    expect(container.querySelectorAll('tbody tr:nth-child(8) td')).toHaveLength(
      8,
    );

    expect(container.innerHTML).toMatchSnapshot();
  });

  test('generateHeaderSpanInfo', () => {

    const groups = [
      [
        'England',
      ],
      [
        'Total',
        'Gender male',
        'Gender female',
        'FSM eligible',
        'FSM not eligible',
        'FSM unclassified',
        'SEN provision No identified SEN',
        'SEN provision statement or EHCP',
        'SEN provision SEN Support',
        'SEN provision Unclassified',
        'SEN provision statement',
        'SEN provision SEN without statement',
      ],
      [
        'Authorised absence rate',
      ],
    ];

    const generated = generateHeaderSpanInfo(groups);

    console.log(generated);

    expect(generated)
      .toStrictEqual([
        [{ heading: 'England', count: 12, start: 0 }],
        [
          { heading: 'Total', count: 1, start: 0 },
          { heading: 'Gender male', count: 1, start: 1 },
          { heading: 'Gender female', count: 1, start: 2 },
          { heading: 'FSM eligible', count: 1, start: 3 },
          { heading: 'FSM not eligible', count: 1, start: 4 },
          { heading: 'FSM unclassified', count: 1, start: 5 },
          { heading: 'SEN provision No identified SEN', count: 1, start: 6 },
          { heading: 'SEN provision statement or EHCP', count: 1, start: 7 },
          { heading: 'SEN provision SEN Support', count: 1, start: 8 },
          { heading: 'SEN provision Unclassified', count: 1, start: 9 },
          { heading: 'SEN provision statement', count: 1, start: 10 },
          { heading: 'SEN provision SEN without statement', count: 1, start: 11 },
        ],
        [{ heading: 'Authorised absence rate', count: 12, start: 0 }]]);


  });

  test('generateHeaderSpanInfo2', () => {

    const groups = [
      [
        'England',
      ],
      [
        'Total',
        'Gender',
        'Gender',
        'FSM',
        'FSM',
        'FSM',
        'SEN provision',
        'SEN provision',
        'SEN provision',
        'SEN provision',
        'SEN provision',
        'SEN provision',
      ],
      [
        'Total',
        'Gender male',
        'Gender female',
        'FSM eligible',
        'FSM not eligible',
        'FSM unclassified',
        'SEN provision No identified SEN',
        'SEN provision statement or EHCP',
        'SEN provision SEN Support',
        'SEN provision Unclassified',
        'SEN provision statement',
        'SEN provision SEN without statement',
      ],
      [
        'Authorised absence rate',
      ],
    ];

    const generated = generateHeaderSpanInfo(groups);

    expect(generated)
      .toStrictEqual([
        [{ heading: 'England', count: 12, start: 0 }],
        [{ heading: 'Total', count: 1, start: 0 },
          { heading: 'Gender', count: 2, start: 1 },
          { heading: 'FSM', count: 3, start: 3 },
          { heading: 'SEN provision', count: 6, start: 6 }],
        [
          { heading: 'Total', count: 1, start: 0 },
          { heading: 'Gender male', count: 1, start: 1 },
          { heading: 'Gender female', count: 1, start: 2 },
          { heading: 'FSM eligible', count: 1, start: 3 },
          { heading: 'FSM not eligible', count: 1, start: 4 },
          { heading: 'FSM unclassified', count: 1, start: 5 },
          { heading: 'SEN provision No identified SEN', count: 1, start: 6 },
          { heading: 'SEN provision statement or EHCP', count: 1, start: 7 },
          { heading: 'SEN provision SEN Support', count: 1, start: 8 },
          { heading: 'SEN provision Unclassified', count: 1, start: 9 },
          { heading: 'SEN provision statement', count: 1, start: 10 },
          { heading: 'SEN provision SEN without statement', count: 1, start: 11 },
        ],
        [{ heading: 'Authorised absence rate', count: 12, start: 0 }]]);


  });

  test('generateHeaders', () => {

    const headers = [
      [{ heading: 'England', count: 12, start: 0 }],
      [{ heading: 'Total', count: 1, start: 0 },
        { heading: 'Gender', count: 2, start: 1 },
        { heading: 'FSM', count: 3, start: 3 },
        { heading: 'SEN provision', count: 6, start: 6 }],
      [
        { heading: 'Total', count: 1, start: 0 },
        { heading: 'Gender male', count: 1, start: 1 },
        { heading: 'Gender female', count: 1, start: 2 },
        { heading: 'FSM eligible', count: 1, start: 3 },
        { heading: 'FSM not eligible', count: 1, start: 4 },
        { heading: 'FSM unclassified', count: 1, start: 5 },
        { heading: 'SEN provision No identified SEN', count: 1, start: 6 },
        { heading: 'SEN provision statement or EHCP', count: 1, start: 7 },
        { heading: 'SEN provision SEN Support', count: 1, start: 8 },
        { heading: 'SEN provision Unclassified', count: 1, start: 9 },
        { heading: 'SEN provision statement', count: 1, start: 10 },
        { heading: 'SEN provision SEN without statement', count: 1, start: 11 },
      ],
      [{ heading: 'Authorised absence rate', count: 12, start: 0 }],
    ];


    const result = generateHeaders(headers);
  });
});
