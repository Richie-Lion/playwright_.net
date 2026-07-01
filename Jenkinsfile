pipeline {
    agent any

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build & Test') {
            steps {
                bat 'dotnet restore'
                bat 'dotnet build'

                bat 'powershell bin\\Debug\\net8.0\\playwright.ps1 install'

                bat 'dotnet test'
            }
        }
    }

    post {
        always {
            // .NET does not generate surefire reports by default
            // Remove JUnit unless you explicitly generate TRX/JUnit XML

            archiveArtifacts artifacts: '**/bin/**/TestResults/*.xml', allowEmptyArchive: true
        }
    }
}