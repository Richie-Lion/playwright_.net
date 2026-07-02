pipeline {
    agent any

    environment {
        PLAYWRIGHT_BROWSERS_PATH = '0'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet restore'
                bat 'dotnet build'
            }
        }

        stage('Install Browsers') {
            steps {
                bat 'dotnet tool install --global Microsoft.Playwright.CLI'
                bat 'playwright install'
            }
        }

        stage('Test') {
            steps {
                bat 'if exist bin\\Debug\\net8.0\\allure-results rmdir /s /q bin\\Debug\\net8.0\\allure-results'
                bat 'dotnet test'
            }
        }
    }
    post {
        always {
            allure results: [[path: 'bin/Debug/net8.0/allure-results']]
        }
    }
}

    