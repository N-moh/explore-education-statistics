import argparse
import subprocess

# NOTE(mark): The slack webhook url, and admin and analyst passwords to access to Admin app are
# stored in the CI pipeline as secret variables, which means they cannot be accessed as normal
# environment variables, and instead must be passed as an argument to this script.


def run_tests_pipeline():
    assert args.admin_password, "Provide an admin password with an '--admin-pass PASS' argument"
    assert args.analyst_password, "Provide an analyst password with an '--analyst-pass PASS' argument"
    assert args.slack_webhook_url, "Please provide slack webhook URL"
    assert args.env, "Provide an environment with an '--env ENV' argument"
    assert args.file, "Provide a file/dir to run with an '--file FILE/DIR' argument"

    valid_environments = ['dev', 'test', 'pre-prod', 'prod', 'ci']

    if args.env not in valid_environments:
        raise Exception(f"Invalid environment provided: {args.env}. Valid environments: {valid_environments}")

    print('Installing dependencies')
    subprocess.check_call(['google-chrome-stable', '--version'])
    # subprocess.check_call(["python3", "-m", "pip install --upgrade pip"])
    # subprocess.check_call(["pip install", "pipenv"])
    # subprocess.check_call(["pipenv", "install"])
    subprocess.run('python -m pip install --upgrade pip', shell=True)
    subprocess.run('pip install pipenv', shell=True)
    subprocess.run('pipenv install', shell=True)

    # utility to not send slack test report if snapshot tests are being run
    def should_send_test_reports() -> bool:
        return args.file != 'tests/general_public/check_snapshots.robot'

    command = f"pipenv run python run_tests.py --admin-pass {args.admin_password} --analyst-pass {args.analyst_password} --slack-webhook-url {args.slack_webhook_url} --env {args.env} --file {args.file} --ci --processes 4 {'--enable-slack' if should_send_test_reports() else None}"

    subprocess.run(command, shell=True)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(
        prog="pipenv run python run_tests_pipeline.py",
        description='Use this script in a CI environment to run UI tests')

    parser.add_argument('--admin-password', dest='admin_password', help='BAU admin password')

    parser.add_argument('--analyst-password', dest='analyst_password', help='Analyst password')

    parser.add_argument(
        '--slack-webhook-url',
        dest='slack_webhook_url',
        help='slack webhook URL for sending test reports'
    )

    parser.add_argument(
        '--env',
        dest='env',
        help=f'environment to run tests against (dev, test, pre-prod, prod & ci)'
    )

    parser.add_argument('--file', dest='file', help='the directory or file to test')

    args = parser.parse_args()
    run_tests_pipeline()