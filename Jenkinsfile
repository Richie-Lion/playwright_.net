pipeline {
    agent any
 
    tools {
        maven 'Maven3'
    }
 
    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }
 
        stage('Build & Test') {
            steps {
                bat 'mvn clean test'
            }
        }
    }
 
    post {
        always {
            junit 'target/surefire-reports/TEST-*.xml'
 
            archiveArtifacts artifacts: 'target/surefire-reports/**', allowEmptyArchive: true
 
            publishHTML(target: [
                reportDir: 'target/surefire-reports',
                reportFiles: 'index.html',
                reportName: 'TestNG HTML Report',
                keepAll: true,
                alwaysLinkToLastBuild: true,
                allowMissing: true
            ])
        }
    }
}