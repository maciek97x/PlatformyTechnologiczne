package net.stawrul.services.exceptions;

/**
 * Wyjątek sygnalizujący puste zamówienie.
 *
 * Wystąpienie wyjątku z hierarchii RuntimeException w warstwie biznesowej
 * powoduje wycofanie transakcji (rollback).
 */
public class EmptyOrderException extends RuntimeException {
}
