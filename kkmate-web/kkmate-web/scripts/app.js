var app = angular.module('kkmate', ['ngRoute']);

app.config(function($routeProvider) {
    $routeProvider
        .when('/', {
            templateUrl: '../templates/home.htm'
        })
        .when('/login', {
            templateUrl: '../templates/login.htm',
            controller: 'loginController'
        });
});