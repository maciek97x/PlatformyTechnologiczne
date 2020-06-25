package net.stawrul.services.exceptions;

/**
 * Wyjątek sygnalizujący zbyt duże zamówienie.
 *
 * Wystąpienie wyjątku z hierarchii RuntimeException w warstwie biznesowej
 * powoduje wycofanie transakcji (rollback).
 */
public class TooManyProductsException extends RuntimeException {
}
